
if (document.readyState == 'loading') {
    document.addEventListener('DOMContentLoaded', ready)
} else {
    ready()
}


function ready() {


    var removeCartItemButtons = $(".close-cart");

    for (var i = 0; i < removeCartItemButtons.length; i++) {
        var button = removeCartItemButtons[i]
        button.addEventListener('click', removeCartItem)
    }

    
    var quantityInputs = document.getElementsByClassName('qty')
    for (var i = 0; i < quantityInputs.length; i++) {
        var input = quantityInputs[i]
        input.addEventListener('change', quantityChangedmincart)
    }



    var addToCartButtons = document.getElementsByClassName('cart-btn')
    cartItems = document.getElementsByClassName('mini-cart-contentul')[0]
   

    
    for (var i = 0; i < addToCartButtons.length; i++) {
        var button = addToCartButtons[i]
        button.addEventListener('click', addToCartClicked)
        
       
  
    }


    // ================================cart===========
    var removeCartItemButtonsForCart = $(".cart-remove-item");
    

    for (var i = 0; i < removeCartItemButtonsForCart.length; i++) {
        var button = removeCartItemButtonsForCart[i]
        button.addEventListener('click', removeCartItemForCart)
    }


    var quantityInputs = document.getElementsByClassName('cart-plus-minus-box')
    for (var i = 0; i < quantityInputs.length; i++) {
        var input = quantityInputs[i]
        input.addEventListener('change', quantityChanged)
    }

    
 


 
    
    



}






count= document.getElementsByClassName("count-cart-after")[0]
    



function addToCartClicked(event) {

    
    // console.log(count)



    
    // countproduct = parseInt(count)
    
    
    
    
    var button = event.target
    var shopItem = button.parentElement.parentElement.parentElement.parentElement
    title = shopItem.getElementsByClassName('product-link')[0].innerText
   
    var price = shopItem.getElementsByClassName('old-price')[0].innerText
    console.log(price)
    
 
    var imageSrc = shopItem.getElementsByClassName('first-img')[0].src
    addItemToCart(title, price, imageSrc)
    updateCartTotal()
   
}

function addItemToCart(title, price, imageSrc) {
    var cartRow = document.createElement('li')
    
    cartRow.classList.add('single-shopping-cart')
   
    
    
    var cartItemNames = cartItems.getElementsByClassName('cartproductname')
    for (var i = 0; i < cartItemNames.length; i++) {
        if (cartItemNames[i].innerText == title) {
            alert('این محصول قبلا به سبد خرید اضافه شده است')
          
            return false ;
        }
    
    }
    var cartRowContents = `

    <div class="shopping-cart-img">
    <a href="single-product.html"><img alt="" src="${imageSrc}"></a>
    <span class="product-quantity">محصول</span>
</div>
<div class="shopping-cart-title">
    <h4><a href="single-product.html" class="cartproductname">${title}</a></h4>
    <span class="cart-price">${price}</span>
    <div class="shopping-cart-delete">
        <a href="#"><i class="ion-android-cancel close-cart"></i></a>
    </div>
</div>
    
            

        `
    cartRow.innerHTML = cartRowContents
    cartItems.append(cartRow)
    
  
    
    
    

    cartRow.getElementsByClassName('close-cart')[0].addEventListener('click', removeCartItem)
    // cartRow.getElementsByClassName('qty')[0].addEventListener('change', quantityChangedmincart)
    addcountproduct()
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
    
}




function removeCartItem(event) {
    var buttonClicked = event.target
    buttonClicked.parentElement.parentElement.parentElement.parentElement.remove()
    updateCartTotal()
    reducecountproduct()
}








function updateCartTotal() {
   
    var cartItemContainer = document.getElementsByClassName('mini-cart-content')[0]
    var cartRows = cartItemContainer.getElementsByClassName('single-shopping-cart')
    var total = 0
    for (var i = 0; i < cartRows.length; i++) {
        var cartRow = cartRows[i]
        var priceElement = cartRow.getElementsByClassName('cart-price')[0]
        // var quantityElement = cartRow.getElementsByClassName('qty')[0]
        var price = parseFloat(priceElement.innerText.replace('تومان ', ''))
        // var quantity = quantityElement.value
        total = total + price 
    }
    total = Math.round(total * 100) / 100
    document.getElementsByClassName('shop-totalprice')[0].innerText = 'تومان ' + total

}


function quantityChangedmincart(event) {
     
    var input = event.target
    if (isNaN(input.value) || input.value <= 0) {
        input.value = 1
    }
    updateCartTotal()
}


function addcountproduct()
{
    

  

    countproductnumber = parseInt(count.innerText)
    


    countproductnumber = countproductnumber +1


    result = countproductnumber.toString()


    count.innerText = result




 
    

}

function reducecountproduct()
{

    countproductnumber = parseInt(count.innerText)


    countproductnumber = countproductnumber -1


    result = countproductnumber.toString()
 

    count.innerText = result

 
}




// ======================cart====================


function removeCartItemForCart(event) {
    var buttonClicked = event.target
    buttonClicked.parentElement.parentElement.parentElement.remove()
    updateCartTotalCart()
  
}

// $('.myremovebtn').click(function (event) {
//     event.preventDefault();
   
//   });


  function quantityChanged(event) {
     
    var input = event.target
    if (isNaN(input.value) || input.value <= 0) {
        input.value = 1
    }
    updateCartTotalCart()
}





  function updateCartTotalCart() {
   
    var cartItemContainer = document.getElementsByClassName('cart-table-content')[0]
    var cartRows = cartItemContainer.getElementsByClassName('cartRows')
    var total = 0
    for (var i = 0; i < cartRows.length; i++) {
        var cartRow = cartRows[i]
        var priceElement = cartRow.getElementsByClassName('amount')[0]
    
        var quantityElement = cartRow.getElementsByClassName('cart-plus-minus-box')[0]
       
        var price = parseFloat(priceElement.innerText.replace(' تومان', ''))
        var quantity = parseInt(quantityElement.value)
      
        total = total + (price * quantity)
    }
    total = Math.round(total * 100) / 100
  
   
    document.getElementsByClassName('pricecarttotal')[0].innerText = ' تومان' + total
    document.getElementsByClassName('pricecarttotal')[1].innerText = ' تومان' + total

}
