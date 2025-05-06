document.addEventListener("DOMContentLoaded", () => {
  const token = localStorage.getItem("token");
  const cartItemsContainer = document.getElementById("cart-items");
  const emptyCartMessage = document.getElementById("empty-cart");
  const cartSummary = document.getElementById("cart-summary");
  const cartTotal = document.getElementById("cart-total");

  async function loadCart() {
    try {
      if (!token) {
        window.location.href = "/login.html";
        return;
      }

      const response = await fetch("http://localhost:5036/api/cart", {
        headers: {
          "Authorization": `Bearer ${token}`
        }
      });

      if (!response.ok) throw new Error("Failed to load cart");

      const cartItems = await response.json();
      
      if (cartItems.length === 0) {
        emptyCartMessage.classList.remove("hidden");
        cartSummary.classList.add("hidden");
        return;
      }

      emptyCartMessage.classList.add("hidden");
      cartSummary.classList.remove("hidden");

      cartItemsContainer.innerHTML = cartItems.map(item => `
        <div class="cart-item flex border-b pb-4" data-id="${item.id}">
          <div class="w-24 h-24 bg-gray-200 rounded overflow-hidden">
            <img src="http://localhost:5036${item.bookImageUrl}" alt="${item.bookTitle}" class="w-full h-full object-cover">
          </div>
          <div class="ml-4 flex-1">
            <h3 class="font-semibold">${item.bookTitle}</h3>
            <p class="text-gray-600">$${item.price.toFixed(2)}</p>
            <div class="flex items-center mt-2">
              <button class="quantity-btn" data-action="decrease">-</button>
              <span class="quantity mx-2">${item.quantity}</span>
              <button class="quantity-btn" data-action="increase">+</button>
              <button class="remove-btn ml-auto text-red-500 hover:text-red-700">
                Remove
              </button>
            </div>
          </div>
        </div>
      `).join("");

      // Calculate total
      const total = cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
      cartTotal.textContent = `$${total.toFixed(2)}`;

      // Add event listeners
      document.querySelectorAll(".quantity-btn").forEach(btn => {
        btn.addEventListener("click", updateQuantity);
      });

      document.querySelectorAll(".remove-btn").forEach(btn => {
        btn.addEventListener("click", removeItem);
      });

    } catch (error) {
      console.error("Error loading cart:", error);
      alert("Failed to load cart. Please try again.");
    }
  }

  async function updateQuantity(e) {
    const btn = e.currentTarget;
    const action = btn.getAttribute("data-action");
    const itemElement = btn.closest(".cart-item");
    const itemId = itemElement.getAttribute("data-id");
    const quantityElement = itemElement.querySelector(".quantity");
    let quantity = parseInt(quantityElement.textContent);

    if (action === "increase") {
      quantity += 1;
    } else if (action === "decrease" && quantity > 1) {
      quantity -= 1;
    }

    try {
      const response = await fetch(`http://localhost:5036/api/cart/${itemId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify({ quantity })
      });

      if (!response.ok) throw new Error("Failed to update quantity");

      quantityElement.textContent = quantity;
      loadCart(); // Refresh cart to update total
    } catch (error) {
      console.error("Error updating quantity:", error);
      alert("Failed to update quantity. Please try again.");
    }
  }

  async function removeItem(e) {
    const itemElement = e.currentTarget.closest(".cart-item");
    const itemId = itemElement.getAttribute("data-id");

    try {
      const response = await fetch(`http://localhost:5036/api/cart/${itemId}`, {
        method: "DELETE",
        headers: {
          "Authorization": `Bearer ${token}`
        }
      });

      if (!response.ok) throw new Error("Failed to remove item");

      itemElement.remove();
      loadCart(); // Refresh cart to check if empty
    } catch (error) {
      console.error("Error removing item:", error);
      alert("Failed to remove item. Please try again.");
    }
  }

  // Initial load
  loadCart();
});
