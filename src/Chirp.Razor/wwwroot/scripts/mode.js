document.addEventListener('DOMContentLoaded', () => {
    const savedTheme = localStorage.getItem('theme') || 'root';
    if (savedTheme === 'light') {
        document.body.classList.add('light_mode');
    }
});

document.getElementById('toggleMode').addEventListener('click', function () {
    event.preventDefault()
    document.body.classList.toggle('light_mode');
    const isLightMode = document.body.classList.contains('light_mode');
    localStorage.setItem('theme', isLightMode ? 'light' : 'root');
    console.log('Button clicked, theme set to:', isLightMode ? 'light' : 'root');
});
