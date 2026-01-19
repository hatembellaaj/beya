import pytest
from selenium import webdriver
from selenium.webdriver.chrome.options import Options


@pytest.fixture()

def driver():
    options = Options()
    options.add_argument("--headless=new")
    options.add_argument("--window-size=1280,720")
    driver_instance = webdriver.Chrome(options=options)
    yield driver_instance
    driver_instance.quit()
