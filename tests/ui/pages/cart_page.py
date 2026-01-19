from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC

from .base_page import BasePage


class CartPage(BasePage):
    CART_ROW = (By.CSS_SELECTOR, "[data-testid='cart-row']")
    CHECKOUT_BUTTON = (By.CSS_SELECTOR, "[data-testid='cart-checkout']")

    def load(self):
        self.open("/cart")

    def has_items(self) -> bool:
        return self.wait_for(EC.presence_of_element_located(self.CART_ROW)) is not None

    def checkout(self):
        self.driver.find_element(*self.CHECKOUT_BUTTON).click()
