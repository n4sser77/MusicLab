window.getAudioDuration = (audioElement) => {
    return audioElement.duration || 100; // Return duration if available
};

window.setAudioPosition = (audioElement, position) => {
    audioElement.currentTime = position;
};
