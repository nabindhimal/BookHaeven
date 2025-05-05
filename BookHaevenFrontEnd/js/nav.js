function isLoggedIn() {
  return !!localStorage.getItem("token");
}

// function getUsernameFromToken() {
//   const token = localStorage.getItem("token");
//   if (!token) return "";

//   try {
//     const payload = JSON.parse(atob(token.split(".")[1]));
//     return payload.name || payload.sub || "User";
//   } catch {
//     return "User";
//   }
// }

// function getUsernameFromToken() {
//   const token = localStorage.getItem("token");
//   if (!token) return "";

//   try {
//     const payload = JSON.parse(atob(token.split(".")[1]));
//     console.log("Decoded JWT Payload:", payload); // 👈 log the decoded token
//     return payload.name || payload.sub || "User";
//   } catch (err) {
//     console.error("Failed to decode token:", err);
//     return "User";
//   }
// }

function getUsernameFromToken() {
  const token = localStorage.getItem("token");
  if (!token) return "";

  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    console.log("Decoded JWT Payload:", payload);
    // use correct claim name
    return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || "User";
  } catch (err) {
    console.error("Failed to decode token:", err);
    return "User";
  }
}



function logout() {
  localStorage.removeItem("token");
  window.location.href = "index.html";
}

document.addEventListener("DOMContentLoaded", () => {
  const nav = document.createElement("nav");
  nav.className = "bg-white shadow p-4 mb-6";

  const container = document.createElement("div");
  container.className = "max-w-5xl mx-auto flex justify-between items-center";

  const left = document.createElement("div");
  left.innerHTML = `<a href="index.html" class="text-2xl font-bold text-blue-600">BookHaeven</a>`;

  const right = document.createElement("div");

  if (isLoggedIn()) {
    const username = getUsernameFromToken();
    right.innerHTML = `
      <span class="mr-4 text-gray-700">Hi, <strong>${username}</strong></span>
      <button class="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600" id="logoutBtn">Logout</button>
    `;
  } else {
    right.innerHTML = `
      <a href="login.html" class="text-blue-600 hover:underline mr-4">Login</a>
      <a href="register.html" class="text-blue-600 hover:underline">Register</a>
    `;
  }

  container.appendChild(left);
  container.appendChild(right);
  nav.appendChild(container);
  document.body.prepend(nav);

  const logoutBtn = document.getElementById("logoutBtn");
  if (logoutBtn) {
    logoutBtn.addEventListener("click", logout);
  }
});

