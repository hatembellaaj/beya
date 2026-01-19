from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC

from .base_page import BasePage


class OrderPage(BasePage):
    ORDER_CONFIRM = (By.CSS_SELECTOR, "[data-testid='order-confirm']")
    ORDER_STATUS = (By.CSS_SELECTOR, "[data-testid='order-status']")

    def confirm_order(self):
        self.wait_for(EC.element_to_be_clickable(self.ORDER_CONFIRM)).click()

    def status_text(self) -> str:
        return self.wait_for(EC.visibility_of_element_located(self.ORDER_STATUS)).text
