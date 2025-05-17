document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".tempAlert").forEach(alertBox => {
        setTimeout(() => {
            alertBox.style.transition = "opacity 0.5s ease";
            alertBox.style.opacity = "0";
            setTimeout(() => {
                alertBox.remove();
            }, 500);
        }, 1500);
    });
});