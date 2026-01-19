from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC

from .base_page import BasePage


class LoginPage(BasePage):
    USERNAME_INPUT = (By.CSS_SELECTOR, "input[placeholder='Utilisateur']")
    PASSWORD_INPUT = (By.CSS_SELECTOR, "input[placeholder='Mot de passe']")
    SUBMIT_BUTTON = (By.XPATH, "//button[normalize-space()='Se connecter']")

    def load(self):
        self.open("/auth")

    def login(self, username: str, password: str):
        self.wait_for(EC.visibility_of_element_located(self.USERNAME_INPUT)).send_keys(username)
        self.driver.find_element(*self.PASSWORD_INPUT).send_keys(password)
        self.driver.find_element(*self.SUBMIT_BUTTON).click()
