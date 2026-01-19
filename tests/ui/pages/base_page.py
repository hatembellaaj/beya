from selenium.webdriver.support.ui import WebDriverWait


class BasePage:
    def __init__(self, driver, base_url: str):
        self.driver = driver
        self.base_url = base_url

    def open(self, path: str):
        self.driver.get(f"{self.base_url}{path}")

    def wait_for(self, condition, timeout: int = 10):
        return WebDriverWait(self.driver, timeout).until(condition)
