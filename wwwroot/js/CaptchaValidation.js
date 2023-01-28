var btn = document.getElementById("registerSubmit");
btn.style.display = "none";

function callback() {

	if (grecaptcha.getResponse().length !== 0) {
		btn.style.display = "block";
	}
}

function recaptchaExpired() {
	btn.style.display = "none";
}