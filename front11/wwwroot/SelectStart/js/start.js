// script.js
document.addEventListener('DOMContentLoaded', function () {
    const enrollButton = document.getElementById('enrollButton');
    const timerDisplay = document.getElementById('timer');

    // ����ѡ�ο�ʼ�ͽ���ʱ��
    const startTime = new Date('2024-06-27T00:00:00');
    const endTime = new Date('2024-06-28T08:30:00');

    function updateCountdown() {
        const now = new Date();
        let remainingTime;

        if (now < startTime) {
            remainingTime = startTime - now;
            timerDisplay.innerText = `����ѡ�ο�ʼ���У�${formatTime(remainingTime)}`;
        } else if (now >= startTime && now <= endTime) {
            remainingTime = endTime - now;
            timerDisplay.innerText = `ѡ�ν�����ʣ��${formatTime(remainingTime)}`;
            enrollButton.disabled = false;
        } else {
            timerDisplay.innerText = 'ѡ���ѽ���';
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
    updateCountdown(); // ��ʼ���������ó�ʼ״̬
});
