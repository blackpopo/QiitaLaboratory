from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By
import urllib.parse
from tqdm import tqdm
from time import sleep
from bs4 import BeautifulSoup

sleep_time = 1
try_max_count = 30
chrome_driver = r'path\to\chromedriver.exe'

headers = {
    'User-Agent': 'Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36'
}

en2ja_url = "https://www.deepl.com/translator#en/ja/"

def get_file(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    return [line.rstrip('\n') for line in lines]

def get_translated_text(text, driver):
    parsed_text = urllib.parse.quote(text)
    url = en2ja_url + parsed_text
    try:
        print(f'url {url}')
        driver.get(url)
        #driver.implicitly_wait(10)
        text = get_text(driver)
        while text == None:
            sleep(2)
            text = get_text(driver)
        return text
    except Exception as e:
        print(f'Error Occurred as {e}')
        return get_translated_text(text, driver)

def get_text(driver):
    try:
        selector = "#dl_translator > div.lmt__text > div.lmt__sides_container > div.lmt__side_container.lmt__side_container--target > div.lmt__textarea_container > div.lmt__translations_as_text > p > button"
        text = driver.find_element_by_css_selector(selector).get_attribute("textContent")
        if text != '':
            return text
        else:
            return None
    except Exception as e:
        print(f'Error occurred as {e}')
        return None

def main():
    options = Options()
    options.add_argument('--headless') #browerが起動せずに実行される
    driver = webdriver.Chrome(chrome_driver, options=options)
    files = ["翻訳したいファイルの一覧"]
    for file in files:
        lines = get_file(file)
        name = file.split('\\')[-1].split('.')[0]
        for line in tqdm(lines):
            sleep(3.0)
            translated = get_translated_text(line, driver)
            print(f'translated text {translated}')
            with open(name + '_translated.txt', 'a', encoding='utf-8') as f:
                f.write(translated + '\n')

if __name__=='__main__':
    main()
