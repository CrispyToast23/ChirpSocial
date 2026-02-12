window.getPeepFromClick = function (x, y) {
    const element = document.elementFromPoint(x, y);
    console.log('Clicked element:', element);
    if (element && element.classList.contains('peep-tag-inline')) {
        const peep = element.getAttribute('data-peep');
        console.log('Found peep:', peep);
        return peep;
    }
    return null;
};

document.addEventListener('DOMContentLoaded', function() {
    console.log('Peep interactions loaded');
    
    document.addEventListener('click', function(e) {
        console.log('Click detected on:', e.target);
        
        if (e.target.classList.contains('peep-tag-inline') && e.target.classList.contains('clickable')) {
            console.log('Peep tag clicked:', e.target.getAttribute('data-peep'));

            e.target.style.transform = 'scale(1.05)';
            setTimeout(() => {
                e.target.style.transform = '';
            }, 200);
        }
    });
});
