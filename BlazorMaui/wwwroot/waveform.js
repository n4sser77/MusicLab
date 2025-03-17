// A basic example


let currentUrl = "";
let wavesurfer;
function loadWaveForm(audioUrl) {
    if (currentUrl !== audioUrl) {

        wavesurfer = WaveSurfer.create({
            container: "#waveform",
            waveColor: 'rgb(200, 0, 200)',
            progressColor: 'rgb(100, 0, 100)',
            url: audioUrl,
            autoplay: false,


        });

        currentUrl = audioUrl;

        wavesurfer.on('ready', () => {
            if (wavesurfer.isPlaying === false) {

                wavesurfer.play()
            }
        });

    }


    if (wavesurfer.isPlaying === true) {
        wavesurfer.pause();
    }

    if (wavesurfer.isPlaying === false) {
        wavesurfer.play();

    }


}