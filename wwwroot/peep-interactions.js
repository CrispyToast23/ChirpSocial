// Function to get peep tag from click coordinates
window.getPeepFromClick = function (x, y) {
    const element = document.elementFromPoint(x, y);
    console.log('Clicked element:', element); // Debug log
    if (element && element.classList.contains('peep-tag-inline')) {
        const peep = element.getAttribute('data-peep');
        console.log('Found peep:', peep); // Debug log
        return peep;
    }
    return null;
};

// Add global event listener for peep tag clicks
document.addEventListener('DOMContentLoaded', function() {
    console.log('Peep interactions loaded'); // Debug log
    
    // Use event delegation on the document
    document.addEventListener('click', function(e) {
        console.log('Click detected on:', e.target); // Debug log
        
        if (e.target.classList.contains('peep-tag-inline') && e.target.classList.contains('clickable')) {
            console.log('Peep tag clicked:', e.target.getAttribute('data-peep')); // Debug log
            
            // The Blazor component will handle the actual navigation
            // This just adds visual feedback
            e.target.style.transform = 'scale(1.05)';
            setTimeout(() => {
                e.target.style.transform = '';
            }, 200);
        }
    });
});
