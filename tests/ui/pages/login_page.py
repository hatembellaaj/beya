from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC

from .base_page import BasePage


class LoginPage(BasePage):
    USERNAME_INPUT = (By.CSS_SELECTOR, "[data-testid='login-username']")
    PASSWORD_INPUT = (By.CSS_SELECTOR, "[data-testid='login-password']")
    SUBMIT_BUTTON = (By.CSS_SELECTOR, "[data-testid='login-submit']")

    def load(self):
        self.open("/login")

    def login(self, username: str, password: str):
        self.wait_for(EC.visibility_of_element_located(self.USERNAME_INPUT)).send_keys(username)
        self.driver.find_element(*self.PASSWORD_INPUT).send_keys(password)
        self.driver.find_element(*self.SUBMIT_BUTTON).click()
