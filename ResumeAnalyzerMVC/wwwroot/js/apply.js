function hideFileName() {
    const fileInput = document.getElementById("file");
    const fileLabel = document.getElementById("fileLabel");

    fileInput.style.display = 'none';

    fileLabel.textContent = '✔️ Resume Uploaded';
}