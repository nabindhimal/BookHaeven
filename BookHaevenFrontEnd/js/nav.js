document.addEventListener("DOMContentLoaded", () => {
  const nav = document.createElement("nav");
  nav.className = "bg-white shadow p-4 mb-6";

  const container = document.createElement("div");
  container.className = "max-w-5xl mx-auto flex justify-between items-center";

  const left = document.createElement("div");
  left.innerHTML = `<a href="index.html" class="text-2xl font-bold text-blue-600">BookHaven</a>`;

  const middle = document.createElement("div");
  middle.className = "flex items-center gap-6"; // Added gap for spacing
  
  // Navigation links (visible to all users)
  middle.innerHTML = `
    <a href="search.html" class="text-gray-700 hover:text-blue-600 transition-colors">Search Books</a>
  `;
  
  // Add Orders link if logged in
  if (isLoggedIn()) {
    middle.innerHTML += `
      <a href="orders.html" class="text-gray-700 hover:text-blue-600 transition-colors">My Orders</a>
    `;
  }


 
  if (isLoggedIn()) {
    middle.innerHTML += `
      <a href="view-bookmark.html" class="text-gray-700 hover:text-blue-600 transition-colors">Bookmarks</a>
    `;
  }

  const right = document.createElement("div");
  right.className = "flex items-center gap-4";
  
  // Cart icon with count (visible to all users)
  right.innerHTML = `
    <a href="cart.html" class="relative flex items-center text-gray-700 hover:text-blue-600">
      <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z"></path>
      </svg>
      <span id="cart-count" class="hidden absolute -top-2 -right-2 bg-red-500 text-white text-xs rounded-full h-5 w-5 flex items-center justify-center">0</span>
    </a>
  `;

  // User section (changes based on auth state)
  if (isLoggedIn()) {
    right.innerHTML += `
      <div class="flex items-center gap-2">
        <span class="text-gray-700">Hi, <strong>${getUsernameFromToken()}</strong></span>
        <button 
          class="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600 transition-colors" 
          id="logoutBtn"
          aria-label="Logout"
        >
          Logout
        </button>
      </div>
    `;
  } else {
    right.innerHTML += `
      <div class="flex items-center gap-4">
        <a href="login.html" class="text-gray-700 hover:text-blue-600 transition-colors">Login</a>
        <a href="register.html" class="text-gray-700 hover:text-blue-600 transition-colors">Register</a>
      </div>
    `;
  }

  container.appendChild(left);
  container.appendChild(middle);
  container.appendChild(right);
  nav.appendChild(container);
  document.body.prepend(nav);

  // Add logout handler if button exists
  const logoutBtn = document.getElementById("logoutBtn");
  if (logoutBtn) {
    logoutBtn.addEventListener("click", logout);
  }

  // Update cart count on page load
  updateCartCount();
});

// Helper functions remain the same
function isLoggedIn() {
  return !!localStorage.getItem("token");
}

function getUsernameFromToken() {
  const token = localStorage.getItem("token");
  if (!token) return "Guest";

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || "User";
  } catch {
    return "User";
  }
}

function logout() {
  localStorage.removeItem("token");
  window.location.href = "index.html";
}

async function updateCartCount() {
  const token = localStorage.getItem("token");
  if (!token) return;

  try {
    const response = await fetch('http://localhost:5036/api/cart/count', {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    if (response.ok) {
      const count = await response.json();
      const cartCountElement = document.getElementById('cart-count');
      if (cartCountElement) {
        cartCountElement.textContent = count;
        cartCountElement.classList.toggle('hidden', count === 0);
      }
    }
  } catch (error) {
    console.error('Error updating cart count:', error);
  }
}