// script.js
document.addEventListener('DOMContentLoaded', function () {
    const enrollButton = document.getElementById('enrollButton');
    const timerDisplay = document.getElementById('timer');

    // 设置选课开始和结束时间
    const startTime = new Date('2024-06-27T00:00:00');
    const endTime = new Date('2024-06-28T08:30:00');

    function updateCountdown() {
        const now = new Date();
        let remainingTime;

        if (now < startTime) {
            remainingTime = startTime - now;
            timerDisplay.innerText = `距离选课开始还有：${formatTime(remainingTime)}`;
        } else if (now >= startTime && now <= endTime) {
            remainingTime = endTime - now;
            timerDisplay.innerText = `选课结束还剩：${formatTime(remainingTime)}`;
            enrollButton.disabled = false;
        } else {
            timerDisplay.innerText = '选课已结束';
            enrollButton.disabled = true;
            clearInterval(timer);
        }
    }

    function formatTime(duration) {
        const seconds = Math.floor((duration / 1000) % 60);
        const minutes = Math.floor((duration / 1000 / 60) % 60);
        const hours = Math.floor((duration / (1000 * 60 * 60)) % 24);
        return `${pad(hours)}:${pad(minutes)}:${pad(seconds)}`;
    }

    function pad(number) {
        return number < 10 ? '0' + number : number;
    }

    const timer = setInterval(updateCountdown, 1000);
    updateCountdown(); // 初始调用以设置初始状态
});
