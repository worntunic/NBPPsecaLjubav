(function($) {
    var $dogEditPopup = $(".dog-edit-popup");
    $dogEditPopup.css("display", "none");
    /*var $dogChangeImage = $("#dogchangeimage-input");
    var $dogImageFile = $("#dogimage-input");
    $("#dogchangeimage-input").on("click", function (event) {
        event.preventDefault();
        let prevChecked = $dogChangeImage.prop("checked");
        let newChecked = !prevChecked;
        $dogChange.prop("checked", newChecked);

        if (newChecked) {
            $dogImageFile.attr("hidden", true);
        } else {
            $dogImageFile.attr("hidden", false);
        }      
    });*/
    $(".dog-edit-popup-close-btn").on("click", function (event) {
        event.preventDefault();
        $dogEditPopup.css("display", "none");
    });
    $(".new-dog-card").on("click", function (event) {
        event.preventDefault();
        $dogEditPopup.css("display", "block");
        $dogEditPopup.find("#dogid-input").val("-1");
    });

    $(".dog-card-manage-icon").on("click", function (event) {
        let $this = $(this);
        let $parent = $this.closest(".dog-card");
        let dogValues = {
            id: $parent.data("id"),
            name: $parent.data("name"),
            race: $parent.data("race"),
            sex: $parent.data("sex"),
            birthdate: $parent.data("birthdate"),
            image: $parent.data("image"),
            adoption: ($parent.data("adoption").toLowerCase() == "true"),
            adopter: ($parent.data("adopter"))
        };
        console.log(dogValues);
        openDogEditWithValues(dogValues);
    });
    function openDogEditWithoutValues() {
        $dogEditPopup.css("display", "block");
        $dogEditPopup.find("#dogid-input").val("-1");
    }
    function openDogEditWithValues(dogValues) {
        $dogEditPopup.css("display", "block");
        $dogEditPopup.find("#dogid-input").val(dogValues.id);
        $dogEditPopup.find("#dogname-input").val(dogValues.name);
        $dogEditPopup.find("#dogsex-input").val(dogValues.sex);
        $dogEditPopup.find("#dograce-input").val(dogValues.race);
        $dogEditPopup.find("#dogadoption-input").prop("checked", dogValues.adoption);
        $dogEditPopup.find("#dogadopter-input").val(dogValues.adopter);
        $dogEditPopup.find("#dogprevimage-input").val(dogValues.image);

        var parts = dogValues.birthdate.split(".");
        var date = new Date(parseInt(parts[2], 10),
            parseInt(parts[1], 10) - 1,
            parseInt(parts[0], 10));
        let dateIso = date.toISOString().substr(0, 10);
        $dogEditPopup.find("#dogbirthdate-input").val(dateIso);

        //$dogChangeImage.attr("hidden", false);
    }
    //Home page confirm adoption
    $(".dog-adopt-popover-icon").on("click", function (event) {
        event.preventDefault();
        let $this = $(this);
        if ($this.hasClass("has-popover")) {
            return;
        } else {
            $this.addClass("has-popover");
        }
        let $dogCard = $this.closest(".dog-card");
        let adopterName = $dogCard.data("adopter");
        let adopterEmail = $dogCard.data("adopteremail");
        let dogID = $dogCard.data("id");
        let dogName = $dogCard.data("name")
        let headHtml = '';
        headHtml += '<div class="adopt-popover-head" class="hide">';
        headHtml += adopterName + ' želi da udomi ' + dogName + '!';
        headHtml += '</div>';
        let contentHtml = '';
        contentHtml += '<div class="adopt-popover-content" class="hide">';
        contentHtml += '<div class="adopt-popover-contact-mail">';
        contentHtml += '<a href="mailto:' + adopterEmail + '">Kontakt</a>';
        contentHtml += '</div>';
        contentHtml += '<div class="row text-center adopt-popover-data-row" id="adopt-popover-' + dogID + '">';
        contentHtml += '<div class="col-md-6 mx-auto text-center">';
        contentHtml += '<div class="btn btn-success btn-sm adopt-confirm">';
        contentHtml += '<i class="fas fa-check"></i>';
        contentHtml += '</div>';
        contentHtml += '</div>';
        contentHtml += '<div class="col-md-6 mx-auto text-center">';
        contentHtml += '<div class="btn btn-danger btn-sm adopt-deny">';
        contentHtml += '<i class="fas fa-times"></i>'
        contentHtml += '</div>';
        contentHtml += '</div>';
        contentHtml += '</div>';
        contentHtml += '</div>';

        $this.popover({
            html: true,
            title: function () {
                return headHtml;
            },
            content: function () {
                return contentHtml;
            }
        });
        $this.popover('show');
    });
    $(document).on("click", ".adopt-confirm", function (event) {
        event.preventDefault();
        $this = $(this);
        let dogID = $this.closest(".adopt-popover-data-row").attr("id");
        dogID = dogID.substring("adopt-popover-".length);
        console.log(dogID);
        $("#adoptiondogid-input").val(dogID);
        $("#adoptionconfirmed-input").val(true);
        $("#confirmadoptdog-form").submit();
    });
    $(document).on("click", ".adopt-deny", function (event) {
        event.preventDefault();
        $this = $(this);
        let dogID = $this.closest(".adopt-popover-data-row").attr("id");
        dogID = dogID.substring("adopt-popover-".length);
        $("#adoptiondogid-input").val(dogID);
        $("#adoptionconfirmed-input").val(false);
        $("#confirmadoptdog-form").submit();
    });
    //Adopt
    $(".adopt-dog-card-overlay").on("click", function (event) {
        event.preventDefault();
        let $this = $(this);
        let id = $this.closest(".dog-card").data("id");
        $("#adoptdogid-input").val(id);
        $("#adoptdog-form").submit();
    });


})(jQuery);

