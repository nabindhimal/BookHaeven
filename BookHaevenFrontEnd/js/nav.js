document.addEventListener("DOMContentLoaded", () => {
  const nav = document.createElement("nav");
  nav.className = "bg-white shadow p-4 mb-6";

  const container = document.createElement("div");
  container.className = "max-w-5xl mx-auto flex justify-between items-center";

  const left = document.createElement("div");
  left.innerHTML = `<a href="index.html" class="text-2xl font-bold text-blue-600">BookHaven</a>`;

  const middle = document.createElement("div");
  middle.innerHTML = `<a href="search.html" class="text-blue-600 hover:underline text-lg">Search Books</a>`;

  const right = document.createElement("div");
  if (isLoggedIn()) {
    right.innerHTML = `
      <span class="mr-4 text-gray-700">Hi, <strong>${getUsernameFromToken()}</strong></span>
      <button 
        class="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600 transition-colors" 
        id="logoutBtn"
        aria-label="Logout"
      >
        Logout
      </button>
    `;
  } else {
    right.innerHTML = `
      <a href="login.html" class="text-blue-600 hover:underline mr-4">Login</a>
      <a href="register.html" class="text-blue-600 hover:underline">Register</a>
    `;
  }

  container.appendChild(left);
  container.appendChild(middle);
  container.appendChild(right);
  nav.appendChild(container);
  document.body.prepend(nav);

  const logoutBtn = document.getElementById("logoutBtn");
  if (logoutBtn) {
    logoutBtn.addEventListener("click", logout);
  }
});

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

