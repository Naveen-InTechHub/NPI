let current = 0;
const total = 5;

function goTo(index) {
    // Hide current panel
    document.getElementById('panel-' + current).classList.remove('active');
    // Update stepper steps
    const steps = document.querySelectorAll('.step');
    steps[current].classList.remove('active');
    if (index > current) steps[current].classList.add('completed');
    else steps[current].classList.remove('completed');

    current = index;

    // Recompute completed classes for all steps
    steps.forEach((s, i) => {
        s.classList.remove('active', 'completed');
        if (i < current) s.classList.add('completed');
        else if (i === current) s.classList.add('active');
        // Update number label
        s.querySelector('.step-num').textContent = i < current ? '✓' : (i + 1);
    });

    // Show new panel
    document.getElementById('panel-' + current).classList.add('active');

    // Update indicator & buttons
    document.getElementById('stepIndicator').textContent = 'Step ' + (current + 1) + ' of ' + total;
    document.getElementById('btnPrev').disabled = current === 0;

    const btnNext = document.getElementById('btnNext');
    if (current === total - 1) {
        btnNext.textContent = 'Finish';
        btnNext.className = 'ans-finish-button';
    } else {
        btnNext.textContent = 'Next';
        btnNext.className = 'ans-primary-button';
    }
}

function changeStep(dir) {
    const next = Math.min(Math.max(current + dir, 0), total - 1);
    goTo(next);
}