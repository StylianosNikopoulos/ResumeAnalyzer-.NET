(function () {
    const serverToken = '@HttpContext.Session.GetString("UserToken")';
    if (serverToken && serverToken !== "null") {
        sessionStorage.setItem("UserToken", serverToken);
    }
})();

function submitKeywords() {
    const input = document.getElementById("keywordInput").value.trim();
    if (!input) {
        alert("Enter keywords.");
        return;
    }
    window.location.href = `/Resumes?keywords=${encodeURIComponent(input)}`;
}
