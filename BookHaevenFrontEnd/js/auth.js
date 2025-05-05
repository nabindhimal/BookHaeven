// js/auth.js
window.saveToken = function(token) {
  localStorage.setItem("token", token);
};

window.getToken = function() {
  return localStorage.getItem("token");
};

window.clearToken = function() {
  localStorage.removeItem("token");
};

