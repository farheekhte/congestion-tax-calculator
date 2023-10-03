# Congestion Tax Calculator

This is a C# application for calculating congestion tax fees for vehicles within the Gothenburg area. The project aims to determine the total congestion tax for vehicles based on the dates and times they pass through specific toll stations.

## The Scenario

Your junior colleague started working on this application, but unfortunately, they have gone on parental leave, leaving the project incomplete. This repository contains the initial codebase with some bugs in the calculation and architecture. Your task is to refactor and complete the code to make it correct and maintainable.

## Assignment

Your mission is to refactor and improve the existing code to meet the following requirements:

- Correctly calculate the total congestion tax for vehicles based on the provided rules.
- Ensure the code is clean, well-organized, and easy to maintain.
- Implement the functionality to handle multiple cities with different congestion tax rules.
- Utilize external data stores for storing and retrieving congestion tax parameters to allow for easy updates by content editors.

## Usage

To use this application, follow these steps:

1. Clone this repository to your local machine.

2. Open the solution in your preferred C# IDE (e.g., Visual Studio).

3. Build the solution to ensure all dependencies are resolved.

4. You can create instances of different types of vehicles (e.g., Car, Motorbike) that implement the `IVehicle` interface and pass them along with a list of DateTime objects representing the dates and times the vehicle passed through toll stations.

5. Call the `CalculateTotalTax` method on an instance of `CongestionTaxCalculator`, passing the vehicle and date/time information as well as the city for which you want to calculate the congestion tax.

6. The method will return the total congestion tax amount for the specified vehicle and city.

7. Ensure that you have the correct parameters for each city in the `_cityTollFees` dictionary or use an external data store as mentioned in the bonus scenario.

## Congestion Tax Rules

- The maximum daily congestion tax per vehicle is 60 SEK.
- Congestion tax is not charged on weekends (Saturdays and Sundays), public holidays, days before a public holiday, and during the month of July.
- Different time intervals have different tax amounts, as specified in the code.

## Bonus Scenario

The codebase is designed to be extensible for handling multiple cities with different congestion tax rules. To achieve this:

1. Move the parameters used by the application to an external data store or configuration file.

2. Implement a mechanism to read these parameters during runtime, allowing different content editors to update the tax rules for various cities.

3. Ensure that the application can handle congestion tax calculations for multiple cities seamlessly.

Feel free to contribute to the project by improving the code, adding features, or providing suggestions for enhancements.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
