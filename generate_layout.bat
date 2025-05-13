@echo off 
echo <!DOCTYPE html> > Views\Shared\_Layout.cshtml 
echo <html lang="en"> >
echo <head> >
echo <meta charset="utf-8"> >
echo <meta name="viewport" content="width=device-width, initial-scale=1.0"> >
echo <title>@ViewData["Title"] - Gyan Ganga</title> >
echo <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700^&display=swap" rel="stylesheet"> >
echo <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet"> >
echo <style> >
echo body { font-family: 'Roboto', sans-serif; background-color: #f8f9fa; color: #333333; margin: 0; padding: 0; } >
echo .sidebar { height: 100vh; width: 250px; position: fixed; top: 0; left: 0; background-color: #007bff; padding-top: 20px; } >
echo .sidebar a { color: #ffffff; padding: 15px 20px; text-decoration: none; display: block; font-weight: 500; } >
echo .sidebar a:hover { background-color: #0056b3; color: #e0e0e0; } >
echo .content { margin-left: 250px; padding: 20px; } >
echo .container { padding-top: 20px; padding-bottom: 20px; } >
echo .card { border: none; box-shadow: 0 2px 10px rgba(0,0,0,0.1); margin-bottom: 20px; } >
echo .btn-primary { background-color: #007bff; border-color: #007bff; } >
echo .btn-primary:hover { background-color: #0056b3; border-color: #0056b3; } >
echo footer { background-color: #007bff; color: #ffffff; padding: 10px 0; text-align: center; position: relative; bottom: 0; width: 100%%; font-size: 0.9rem; margin-left: 250px; } >
echo footer a { color: #e0e0e0; margin: 0 5px; text-decoration: none; } >
echo footer a:hover { color: #ffffff; } >
echo footer p { margin: 0; } >
echo </style> >
echo </head> >
echo <body> >
echo <div class="sidebar"> >
echo <a href="@Url.Action("Index", "Home")">Home</a> >
echo <a href="@Url.Action("Index", "Books")">Books</a> >
echo <a href="@Url.Action("Cart", "Books")">Cart</a> >
echo <a href="@Url.Action("Saved", "Books")">Saved Books</a> >
echo @if (User?.Identity?.IsAuthenticated == true) { >
echo     <a href="@Url.Action("Logout", "Account")">Logout</a> >
echo } else { >
echo     <a href="@Url.Action("Login", "Account")">Login</a> >
echo     <a href="@Url.Action("Register", "Account")">Register</a> >
echo } >
echo </div> >
echo <div class="content"> >
echo     <div class="container"> >
echo         @RenderBody() >
echo     </div> >
echo </div> >
echo <footer> >
echo     <div class="container"> >
echo         <p>(C) 2025 Gyan Ganga | <a href="@Url.Action("Index", "Home")">Home</a> | <a href="@Url.Action("Index", "Books")">Books</a> | <a href="@Url.Action("Cart", "Books")">Cart</a> | <a href="@Url.Action("Saved", "Books")">Saved Books</a></p> >
echo     </div> >
echo </footer> >
echo <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script> >
echo @RenderSection("Scripts", required: false) >
echo </body> >
echo </html> >
