window.onload = function() {
    let savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
    document.getElementById('themeStyle').href = savedTheme;
    }
}

document.getElementById('toggleMode').addEventListener('click', function () {
    var link = document.getElementById('themeStyle');
    var newTheme = link.href.includes('lightmode.css') ? '/css/style.css' : '/css/lightmode.css';
    
    link.href = newTheme;
    
    localStorage.setItem('theme', newTheme);
});
