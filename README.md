```
Author:     Phuc Hoang
Partner:    Chanphone Visathip
Start Date: 10-Jan-2024
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  PhucHoang123
Repo:       https://github.com/uofu-cs3500-spring24/assignment-six-gui-functioning-spreadsheet-v_tekkkkk
Commit Date: 1-16-2024 11:25pm
Solution:   Spreadsheet
Copyright:  CS 3500 and Phuc Hoang - This work may not be copied for use in Academic Coursework.
```

# Overview of the Spreadsheet functionality

The Spreadsheet program is currently capable of calculate basic math function with using infix notation.
Future extensions will be added

# Time Expenditures:

    1. Assignment One:      Predicted Hours:          10        Actual Hours:   7
    2. Assignment Two:      Predicted Hours:          20        Actual Hours:   9
    3. Assignment three:    Predicted Hours:          20        Actual Hours:   10
    4. Assignment Four:     Predicted Hours:          20        Actual Hours:   11
    5. Assignment Five:     Predicted Hours:          20        Actual Hours:   24
    6. Assignment Six:      Predicted Hours:          30        Actual Hours:   35

# Good software practice (GSP)
    1.DRY : 
             We used a lot of helper short methods that do a specific things that most of methods need. For example, GetCellName() in MyEntry class. We declare it as internal class
             so that we can use in MainPage class.
    2.Well-named, commented and short methods : 
             For instance, the ShowSelectedCell(), ShowContent(), and ChangeContent() methods 
             in the MainPage class have clear names that indicate their purpose, and they are commented to provide additional context. 
             Keeping methods short and focused on a specific task enhances readability, understandability, and maintainability of the codebase.
    3.Separation of Concerns: 
            The code separates different concerns into individual methods and classes. For example, the MainPage class is responsible for managing the user interface and interaction, 
            while the MyEntry class handles every individual spreadsheet cells such as updated value of cells and highlight selected cell, 
            This separation makes the code easier to understand, maintain, and test.
    4. Code Reuse:
            he code demonstrates code reuse by utilizing existing classes and libraries. For example, the MainPage class leverages the Spreadsheet class to handle spreadsheet operations, 
            such as creating a new spreadsheet, opening an existing one, and saving changes. This reuse of functionality reduces redundancy, promotes consistency, and simplifies maintenance.
            

            
        
