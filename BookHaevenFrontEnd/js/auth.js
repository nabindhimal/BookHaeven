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

window.parseJwt = function (token) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    return null;
  }
};


