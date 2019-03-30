var countDownDate = new Date();
countDownDate.setTime(countDownDate.getTime() + (15 * 60 * 1000)); // add 15 minutes

var x = setInterval(function () {

    // Get todays date and time
    var now = new Date().getTime();

    // Find the distance between now and the count down date
    var distance = countDownDate - now;

    // Time calculations for days, hours, minutes and seconds
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);

    // Display the result in the element with id="demo"
    document.getElementById("timeLeft").innerText = minutes + "m " + seconds + "s ";

    // If the count down is finished, write some text
    if (distance < 0) {
        clearInterval(x);
        document.getElementById("timeLeft").innerHTML = "Displaying now:";

        $.get("/OrdersFunction2.txt", function (data) {
            $("#help").html(PR.prettyPrintOne(data));
            document.getElementById("help").style.display = "";
        });
    }
}, 1000);