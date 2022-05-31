using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using lib_audio_analysis;
using System.Threading;
using System.Threading.Tasks;
class AudioManager : MonoBehaviour
{
    InputCaptureFuncs inputCap;

    //入力デバイスの設定はここで入力 
    ushort inputChannels = 1;
    public uint SamplingRate { get; private set; } 
    public float[] DataSamples { get; private set; }
    int frameBufferSize = 1024;

    int captureBufferSize;
    byte[] captureData;
    IntPtr captureDataPtr;

    List<float> waveBuffer;

    bool cap_now;

    Thread rcv_wave_thread;

    Thread create_clip_thread;

    double typeMax;

    BitRate inputBitRate;

    AudioClip audioClip;

    AudioSource audioSource;

    bool data_flag;

    [SerializeField] float gain = 1.0f;


    public struct Int24
    {
        public Int24(Int32 data) { Value = data; }
        static public Int32 max() { return (Int32)Math.Pow(2.0, 24.0) / 2 - 1; }
        static public Int32 min() { return -1 * (Int32)Math.Pow(2.0, 24.0) / 2; }
        public Int32 Value { get; set; }
    }
    public enum BitRate 
    {
        Integer16 = 16,
        Integer24 = 24,
        Integer32 = 32,
    } ;
    async void Start()
    {
        inputCap = new InputCaptureFuncs();


        SamplingRate = 48000;
        inputChannels = 1;
        inputBitRate = BitRate.Integer16;

        //typeMaxの初期化
        typeMax = Int16.MaxValue;

        cap_now = false;
        //自環境で試した結果、8.0ms取得、512サンプル処理が一番遅延が少ないため、今回はこの設定で行う
        long hr = inputCap.initInputCapture(SamplingRate, inputChannels, (ushort)inputBitRate, 20, 0);
        waveBuffer = new List<float>();
        DataSamples = new float[frameBufferSize];

        captureBufferSize = inputCap.getDataBufferSize();
        Debug.Log("Capture Buffer SIze" + captureBufferSize); //96000
        captureData = new byte[captureBufferSize];
        captureDataPtr = new IntPtr();
        captureDataPtr = Marshal.AllocCoTaskMem(captureBufferSize);
        inputCap.startCapture();
        
        cap_now = true;

        audioSource = GetComponent<AudioSource>();

        // GetMic();

        audioSource.clip = AudioClip.Create("mic", (int)frameBufferSize, inputChannels, (int)SamplingRate, false, false);

        audioSource.loop = true;

        audioSource.Play();

        rcv_wave_thread =  new Thread(new ThreadStart(capture));
        rcv_wave_thread.Start();

        create_clip_thread = new Thread(new ThreadStart(play));
        create_clip_thread.Start();
    }


    void capture()
    {
        while(true)
        {
            int size = 0;
            long hr = inputCap.getCaptureData(ref captureDataPtr, ref size);
            if (hr == 0)
            {
                Marshal.Copy(captureDataPtr, captureData, 0, size);
                inputSoundDataConvert(captureData, size);
            }
            else
            {
                if(!cap_now) 
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        cap_now = false;
        rcv_wave_thread.Join();
        rcv_wave_thread = null;
        create_clip_thread.Join();
        create_clip_thread = null;
        inputCap.stopCapture();
        inputCap = null;
    }

    private void inputSoundDataConvert(byte[] captureData, int size)
    {
        int captureDataIncremation = 16 / 8 * inputChannels;
        for (int i = 0; i < size; i += captureDataIncremation)
        {
            byte[] tmp = new byte[] { captureData[i], captureData[i + 1] };
            waveBuffer.Add((float)BitConverter.ToInt16(tmp, 0) / 32767.0f);
        }
    }

    private void play()
    {
        while(cap_now)
        {
            if (waveBuffer.Count > frameBufferSize)
            {
                DataSamples = waveBuffer.GetRange(0, frameBufferSize).ToArray();          
                waveBuffer.RemoveRange(0, frameBufferSize);
                data_flag = true;
            }
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
            int dataLen = data.Length / channels;

            for (int i = 0; i < dataLen ; i++){
                data[2 * i] = DataSamples[i] * gain;
                data[2 * i + 1] = DataSamples[i] * gain;
            }
    }

    private void GetMic()
    {
        while (Microphone.devices.Length< 1) { }
        Debug.Log("Mic GET!!!!");
        string device = Microphone.devices[0];
        audioSource.loop = true;
        audioSource.clip = Microphone.Start(device, true, 1, (int)SamplingRate);
        while (!(Microphone.GetPosition(device) > 0)) { }
        audioSource.Play();
    }
}