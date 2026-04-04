window._steamCharts = {};

// opts: { logScale: bool, yMin: number }
window.renderSteamChart = function (canvasId, chartType, labels, values, label, opts) {
    if (window._steamCharts[canvasId]) {
        window._steamCharts[canvasId].destroy();
        delete window._steamCharts[canvasId];
    }

    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    opts = opts || {};
    const isDark    = document.body.classList.contains('dark-mode');
    const textColor = isDark ? '#c6d4df' : '#374151';
    const gridColor = isDark ? 'rgba(255,255,255,0.08)' : 'rgba(0,0,0,0.08)';
    const isRadial  = chartType === 'doughnut' || chartType === 'pie';

    const dataset = isRadial
        ? {
            label,
            data: values,
            backgroundColor: [
                '#66c0f4','#1b8dd4','#4c9fd4','#2a475e','#4c6b8a',
                '#90caf9','#42a5f5','#1565c0','#0d47a1','#bbdefb',
                '#0288d1','#01579b','#4fc3f7'
            ],
            borderWidth: 1,
            borderColor: isDark ? '#1b2838' : '#ffffff'
        }
        : {
            label,
            data: values,
            backgroundColor: '#66c0f4cc',
            borderColor: '#1b8dd4',
            borderWidth: 2,
            borderRadius: 4,
            fill: chartType === 'line',
            tension: 0.35,
            pointBackgroundColor: '#1b8dd4',
            pointRadius: chartType === 'line' ? 4 : 0
        };

    const scaleType = opts.logScale ? 'logarithmic' : 'linear';

    window._steamCharts[canvasId] = new Chart(canvas, {
        type: chartType,
        data: { labels, datasets: [dataset] },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: isRadial,
                    labels: { color: textColor, boxWidth: 14, font: { size: 12 } }
                },
                tooltip: {
                    callbacks: {
                        label: ctx => {
                            const val = ctx.parsed.y ?? ctx.parsed;
                            return ` ${ctx.dataset.label}: ${typeof val === 'number' ? val.toLocaleString() : val}`;
                        }
                    }
                }
            },
            scales: isRadial ? {} : {
                x: {
                    ticks: {
                        color: textColor,
                        maxRotation: 40,
                        autoSkip: true,
                        maxTicksLimit: 12,
                        font: { size: 11 }
                    },
                    grid: { color: gridColor }
                },
                y: {
                    type: scaleType,
                    min: opts.yMin ?? undefined,
                    ticks: {
                        color: textColor,
                        autoSkip: true,
                        maxTicksLimit: 6,
                        font: { size: 11 },
                        callback: val => {
                            if (val >= 1_000_000) return (val / 1_000_000).toFixed(1) + 'M';
                            if (val >= 1_000)     return (val / 1_000).toFixed(0) + 'K';
                            return val;
                        }
                    },
                    grid: {
                        color: gridColor,
                        drawTicks: false
                    }
                }
            }
        }
    });
};

window.saveFilterSettings = function (settings) {
    try { localStorage.setItem('steamDashboardFilters', JSON.stringify(settings)); } catch {}
};

window.loadFilterSettings = function () {
    try {
        const s = localStorage.getItem('steamDashboardFilters');
        return s ? JSON.parse(s) : null;
    } catch { return null; }
};
