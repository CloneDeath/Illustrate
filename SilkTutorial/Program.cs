using System;
using SilkTutorial;

try {
    var app = new HelloTriangleApplication();
    app.Run();
}
catch (Exception ex) {
    Console.WriteLine(ex);
    throw;
}