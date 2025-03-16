document.addEventListener('DOMContentLoaded', function() {
    const hamburger = document.querySelector('.hamburger');
    const navMenu = document.querySelector('nav ul');
    const filterButtons = document.querySelector('.filter-buttons');
    const projects = document.querySelectorAll('.project');
    const lightbox = document.createElement('div');
    const form = document.querySelector('form');

    lightbox.id = 'lightbox';
    document.body.appendChild(lightbox);

    hamburger.addEventListener('click', function() {
        navMenu.classList.toggle('visible');
    });

    // Smooth scrolling
    navMenu.addEventListener('click', function(e) {
        if (e.target.tagName === 'A') {
            e.preventDefault();
            const targetId = e.target.getAttribute('href').substring(1);
            document.getElementById(targetId).scrollIntoView({
                behavior: 'smooth'
            });
        }
    });

    // Filter feature
    filterButtons.addEventListener('click', function(e) {
        if (e.target.classList.contains('filter-button')) {
            const filter = e.target.getAttribute('data-filter');
            projects.forEach(project => {
                if (filter === 'all' || project.classList.contains(filter)) {
                    project.style.display = 'block';
                } else {
                    project.style.display = 'none';
                }
            });
        }
    });

    // Lightbox effect
    document.querySelector('#projects').addEventListener('click', function(e) {
        if (e.target.tagName === 'IMG') {
            lightbox.classList.add('active');
            const img = document.createElement('img');
            img.src = e.target.src;
            while (lightbox.firstChild) {
                lightbox.removeChild(lightbox.firstChild);
            }
            lightbox.appendChild(img);
        }
    });

    lightbox.addEventListener('click', function() {
        lightbox.classList.remove('active');
    });

    // Form validation
    form.addEventListener('submit', function(e) {
        const name = document.getElementById('name').value.trim();
        const email = document.getElementById('email').value.trim();
        const message = document.getElementById('message').value.trim();

        if (!name || !email || !message) {
            e.preventDefault();
            alert('Please fill in all fields.');
        }
    });
});
