document.addEventListener('DOMContentLoaded', function () {

    // Utility functions
    const debounce = (func, wait) => {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    };

    // Enhanced mobile menu functionality
    function initMobileMenu() {
        const navbarToggler = document.querySelector('.navbar-toggler');
        const navbarCollapse = document.querySelector('.navbar-collapse');
        const navLinks = document.querySelectorAll('.navbar-nav .nav-link');

        // Add click sound effect (optional)
        const playClickSound = () => {
            // You can add a subtle click sound here if desired
            // const audio = new Audio('/sounds/click.mp3');
            // audio.play().catch(() => {}); // Ignore errors if sound fails
        };

        // Enhanced mobile menu toggle with animation
        if (navbarToggler && navbarCollapse) {
            navbarToggler.addEventListener('click', function () {
                playClickSound();

                // Add/remove active class for animation
                this.classList.toggle('active');

                // Add staggered animation to menu items
                setTimeout(() => {
                    const menuItems = navbarCollapse.querySelectorAll('.nav-item');
                    menuItems.forEach((item, index) => {
                        setTimeout(() => {
                            item.style.transform = 'translateX(0)';
                            item.style.opacity = '1';
                        }, index * 100);
                    });
                }, 150);
            });

            // Auto-close mobile menu on outside click
            document.addEventListener('click', function (e) {
                if (!navbarToggler.contains(e.target) && !navbarCollapse.contains(e.target)) {
                    if (navbarCollapse.classList.contains('show')) {
                        navbarToggler.click();
                    }
                }
            });

            // Auto-close on link click (mobile only)
            navLinks.forEach(link => {
                link.addEventListener('click', function () {
                    if (window.innerWidth < 992 && navbarCollapse.classList.contains('show')) {
                        setTimeout(() => navbarToggler.click(), 150);
                    }
                });
            });
        }
    }

    // Enhanced scroll effects
    function initScrollEffects() {
        const navbar = document.querySelector('.navbar');
        let lastScrollTop = 0;
        let scrollTimeout;

        const handleScroll = () => {
            const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

            // Add/remove scrolled class
            if (scrollTop > 50) {
                navbar?.classList.add('scrolled');
            } else {
                navbar?.classList.remove('scrolled');
            }

            // Optional: Hide/show navbar on scroll direction change
            if (scrollTop > lastScrollTop && scrollTop > 200) {
                navbar?.classList.add('nav-hidden');
            } else {
                navbar?.classList.remove('nav-hidden');
            }

            lastScrollTop = scrollTop <= 0 ? 0 : scrollTop;
        };

        // Throttled scroll handler
        const throttledScroll = debounce(handleScroll, 10);
        window.addEventListener('scroll', throttledScroll);
    }

    // Enhanced button interactions
    function initButtonEffects() {
        const buttons = document.querySelectorAll('.btn:not(.no-ripple)');

        buttons.forEach(button => {
            button.addEventListener('click', function (e) {
                // Create ripple effect
                const ripple = document.createElement('span');
                const rect = this.getBoundingClientRect();
                const size = Math.max(rect.width, rect.height);
                const x = e.clientX - rect.left - size / 2;
                const y = e.clientY - rect.top - size / 2;

                ripple.classList.add('ripple-effect');
                ripple.style.width = ripple.style.height = size + 'px';
                ripple.style.left = x + 'px';
                ripple.style.top = y + 'px';

                this.appendChild(ripple);

                setTimeout(() => {
                    ripple.remove();
                }, 600);
            });
        });
    }

    // Enhanced form interactions
    function initFormEffects() {
        const formControls = document.querySelectorAll('.form-control, .form-select');

        formControls.forEach(control => {
            // Focus effects
            control.addEventListener('focus', function () {
                this.closest('.form-group, .mb-3, .form-floating')?.classList.add('focused');
            });

            control.addEventListener('blur', function () {
                this.closest('.form-group, .mb-3, .form-floating')?.classList.remove('focused');

                // Add filled class if has value
                if (this.value.trim() !== '') {
                    this.classList.add('filled');
                } else {
                    this.classList.remove('filled');
                }
            });
        });
    }

    // Enhanced image loading with lazy loading fallback
    function initImageEffects() {
        // Simple image error handling without loading animations
        const images = document.querySelectorAll('img');

        images.forEach(img => {
            img.onerror = () => {
                img.src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjBmMGYwIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtZmFtaWx5PSJBcmlhbCwgc2Fucy1zZXJpZiIgZm9udC1zaXplPSIxOCIgZmlsbD0iIzk5OTk5OSIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPkltYWdlIG5vdCBhdmFpbGFibGU8L3RleHQ+PC9zdmc+';
            };
        });
    }

    // Enhanced animations with intersection observer
    function initAnimations() {
        const animatedElements = document.querySelectorAll('.fade-in, .hover-lift, .product-card, .category-card');

        const animationObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');

                    // Staggered animation for product grids
                    if (entry.target.parentElement?.classList.contains('product-grid')) {
                        const siblings = Array.from(entry.target.parentElement.children);
                        const index = siblings.indexOf(entry.target);
                        entry.target.style.animationDelay = `${index * 100}ms`;
                    }
                }
            });
        }, {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        });

        animatedElements.forEach(el => animationObserver.observe(el));
    }

    // Enhanced search functionality (if search is implemented)
    function initSearch() {
        const searchIcon = document.querySelector('.fa-search');
        const searchInput = document.querySelector('#search-input');

        if (searchIcon) {
            searchIcon.addEventListener('click', function () {
                if (searchInput) {
                    searchInput.focus();
                } else {
                    // Create inline search if not exists
                    const searchBar = document.createElement('div');
                    searchBar.innerHTML = `
                        <div class="search-overlay">
                            <div class="search-container">
                                <input type="text" class="form-control search-input" placeholder="Search products..." autofocus>
                                <button class="btn btn-link search-close">&times;</button>
                            </div>
                        </div>
                    `;
                    document.body.appendChild(searchBar);

                    // Close search on button click or escape key
                    searchBar.querySelector('.search-close').addEventListener('click', () => {
                        searchBar.remove();
                    });

                    document.addEventListener('keydown', function (e) {
                        if (e.key === 'Escape') {
                            searchBar.remove();
                        }
                    });
                }
            });
        }
    }

    // Performance monitoring
    function initPerformanceMonitoring() {
        // Monitor page load time
        window.addEventListener('load', function () {
            setTimeout(() => {
                const loadTime = performance.timing.loadEventEnd - performance.timing.navigationStart;
                if (loadTime > 3000) {
                    console.warn('Page load time is slower than expected:', loadTime + 'ms');
                }
            }, 0);
        });

        // Monitor scroll performance
        let scrollTicking = false;
        window.addEventListener('scroll', function () {
            if (!scrollTicking) {
                requestAnimationFrame(function () {
                    // Scroll performance optimizations
                    scrollTicking = false;
                });
                scrollTicking = true;
            }
        });
    }

    // Accessibility enhancements
    function initAccessibility() {
        // Add skip to main content link
        if (!document.querySelector('.skip-to-main')) {
            const skipLink = document.createElement('a');
            skipLink.href = '#main-content';
            skipLink.className = 'skip-to-main sr-only sr-only-focusable';
            skipLink.textContent = 'Skip to main content';
            document.body.insertAdjacentElement('afterbegin', skipLink);
        }

        // Enhance keyboard navigation
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Tab') {
                document.body.classList.add('keyboard-nav');
            }
        });

        document.addEventListener('mousedown', function () {
            document.body.classList.remove('keyboard-nav');
        });

        // Add ARIA labels to interactive elements without them
        const interactiveElements = document.querySelectorAll('button:not([aria-label]), a:not([aria-label])');
        interactiveElements.forEach(el => {
            if (!el.getAttribute('aria-label') && !el.textContent.trim()) {
                const icon = el.querySelector('i[class*="fa-"]');
                if (icon) {
                    const iconClass = Array.from(icon.classList).find(cls => cls.startsWith('fa-'));
                    if (iconClass) {
                        const label = iconClass.replace('fa-', '').replace('-', ' ');
                        el.setAttribute('aria-label', label);
                    }
                }
            }
        });
    }

    // Initialize all features
    function init() {
        try {
            initMobileMenu();
            initScrollEffects();
            initButtonEffects();
            initFormEffects();
            initImageEffects();
            initAnimations();
            initSearch();
            initPerformanceMonitoring();
            initAccessibility();

            console.log('Site enhancements initialized successfully');
        } catch (error) {
            console.error('Error initializing site enhancements:', error);
        }
    }

    // Run initialization
    init();

    // Re-initialize on dynamic content changes (if using AJAX)
    const observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            if (mutation.addedNodes.length) {
                // Re-initialize specific features for new content
                initButtonEffects();
                initFormEffects();
                initImageEffects();
            }
        });
    });

    observer.observe(document.body, { childList: true, subtree: true });
});

// Additional CSS for animations and effects
const additionalStyles = `
    .ripple-effect {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.6);
        transform: scale(0);
        animation: ripple-animation 0.6s linear;
        pointer-events: none;
    }
    
    @keyframes ripple-animation {
        to {
            transform: scale(4);
            opacity: 0;
        }
    }
    
    .nav-hidden {
        transform: translateY(-100%);
        transition: transform 0.3s ease;
    }
    
    .loading {
        opacity: 0.7;
        filter: blur(1px);
    }
    
    .loaded {
        opacity: 1;
        filter: none;
        transition: all 0.3s ease;
    }
    
    .animate-in {
        animation: slideInUp 0.6s ease forwards;
    }
    
    @keyframes slideInUp {
        from {
            opacity: 0;
            transform: translateY(30px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
    
    .search-overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.8);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 9999;
        animation: fadeIn 0.3s ease;
    }
    
    .search-container {
        display: flex;
        align-items: center;
        max-width: 500px;
        width: 90%;
    }
    
    .search-input {
        font-size: 1.2rem;
        padding: 1rem;
        border: none;
        border-radius: 0.5rem 0 0 0.5rem;
    }
    
    .search-close {
        background: white;
        border-radius: 0 0.5rem 0.5rem 0;
        font-size: 2rem;
        line-height: 1;
        padding: 1rem;
        margin-left: -1px;
    }
    
    .skip-to-main {
        position: absolute;
        top: -40px;
        left: 6px;
        background: var(--primary-color);
        color: white;
        padding: 8px;
        border-radius: 4px;
        text-decoration: none;
        z-index: 10000;
    }
    
    .skip-to-main:focus {
        top: 6px;
    }
    
    .keyboard-nav *:focus {
        outline: 2px solid var(--primary-color);
        outline-offset: 2px;
    }
    
    @media (prefers-reduced-motion: reduce) {
        .animate-in {
            animation: none;
        }
        
        .ripple-effect {
            display: none;
        }
    }
`;

// Inject additional styles
const styleSheet = document.createElement('style');
styleSheet.textContent = additionalStyles;
document.head.appendChild(styleSheet);