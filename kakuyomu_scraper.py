import requests
from bs4 import BeautifulSoup
from sklearn.neighbors import VALID_METRICS
from tqdm import tqdm
from time import sleep

headers = {
    'User-Agent': 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.76 Safari/537.36'}
url_header = 'https://kakuyomu.jp/works'

def get_text(url, gyokan=False):
    print(f'Start each page at {url}')
    response = requests.get(url, headers=headers)
    soup = BeautifulSoup(response.content, 'html.parser')
    # print(soup)
    text = soup.find(class_='widget-episode-inner')
    texts = [line.text for line in text.find_all('p')]
    texts = [line.replace('\u3000', '') for line in texts]
    if not gyokan:
        texts = [line for line in texts if line != '']
    return texts

def get_page(code, gyokan=False):
    url =  url_header+ '/' + code 
    print(f'url : {url}')
    response = requests.get(url, headers=headers)
    soup = BeautifulSoup(response.content, 'html.parser')

    title = soup.find(id="workTitle")
    title = title.get_text()
    author = soup.find(id="workAuthor")
    author = author.get_text()
    indices = soup.find(class_='widget-toc-main')
    if indices == None:
        raise ValueError(f"Indices not found... at {url}")
    else:
        hrefs = ['/'.join(a['href'].split('/')[-3:]) for a in indices.find_all('a')]
        #hrefs = sorted(hrefs, key=lambda x: int(x.split('/')[-1]))
        print(f'length of hrefs {len(hrefs)}')
        for href in tqdm(hrefs):
            url = url_header + '/' + href
            lines = get_text(url, gyokan)
            sleep(1.0)
            with open(code + '.txt', 'a', encoding='utf-8') as f:
                f.write('\n'.join(lines) + "\n")
    rename_file(code, title, author)

import os
import re
invalid_string = re.compile(r'[\\/:*?"<<>|]+')
def rename_file(code, title, author):
    title = invalid_string.sub('', title)
    author = author.split('\n')[-2]
    author = invalid_string.sub('', author)
    os.rename(code + '.txt', f'{title}_{author}.txt')

import argparse

if __name__=='__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--url', type=str)

    args = parser.parse_args()
    code = args.url.split('/')[-1]
    print(f'code {code}')
    get_page(code, gyokan=False)