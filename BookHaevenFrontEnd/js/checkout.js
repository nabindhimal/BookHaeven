document.addEventListener("DOMContentLoaded", async () => {
    const checkoutBtn = document.getElementById("checkout-btn");
    if (checkoutBtn) {
        checkoutBtn.addEventListener("click", processCheckout);
    }
});

async function processCheckout() {
    const token = localStorage.getItem("token");
    if (!token) {
        window.location.href = "/login.html";
        return;
    }

    try {
        // Show loading state
        const checkoutBtn = document.getElementById("checkout-btn");
        checkoutBtn.disabled = true;
        checkoutBtn.textContent = "Processing...";
        checkoutBtn.classList.add("opacity-75");

        const response = await fetch("http://localhost:5036/api/orders/checkout", {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            const error = await response.json().catch(() => ({}));
            throw new Error(error.message || "Checkout failed. Please try again.");
        }

        const order = await response.json();
        
        // Immediately redirect to confirmation page with order ID
        window.location.href = `/order-confirmation.html?id=${order.id}`;

    } catch (error) {
        console.error("Checkout error:", error);
        
        // Reset button state
        const checkoutBtn = document.getElementById("checkout-btn");
        if (checkoutBtn) {
            checkoutBtn.disabled = false;
            checkoutBtn.textContent = "Proceed to Checkout";
            checkoutBtn.classList.remove("opacity-75");
        }

        // Show error message
        showErrorMessage(error.message);
    }
}

function showErrorMessage(message) {
    // Create a more user-friendly error display
    const errorDiv = document.createElement("div");
    errorDiv.className = "mt-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded";
    errorDiv.innerHTML = `
        <strong>Checkout Failed:</strong>
        <span class="block">${message}</span>
    `;
    
    const cartSummary = document.getElementById("cart-summary");
    if (cartSummary) {
        cartSummary.appendChild(errorDiv);
        
        // Auto-remove error after 5 seconds
        setTimeout(() => {
            errorDiv.remove();
        }, 5000);
    } else {
        // Fallback to alert if DOM element not found
        alert(`Error: ${message}`);
    }
}
