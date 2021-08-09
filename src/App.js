import Expenses from "./components/Expenses/Expenses";
import NewExpense from "./components/NewExpense/NewExpense";
import AddUser from "./components/AddUser/AddUser";
import UserList from './components/UserList/UserList';
import { useState } from "react";

let DUMMY_EXPENSES = [
{
    id: "e1",
    title: "Toilet Paper",
    amount: 94.12,
    date: "2022-02-17T09:45:40",
  },
  { id: "e2", title: "New TV", amount: 799.49, date: "2021-06-13T19:03:40" },
  {
    id: "e3",
    title: "Car Insurance",
    amount: 294.67,
    date: "2022-03-25T17:55:00",
  },
  {
    id: "e4",
    title: "New Desk (Wooden)",
    amount: 450,
    date: "2021-04-08T09:25:00",
  },
  {
    id: "e5",
    title: "Cycling gloves",
    amount: 20,
    date: "2019-05-23T17:40:00",
  }
];

function App() {

  //const [expenses, setExpenses] = useState(DUMMY_EXPENSES);

  // const addExpenseHandler = (expense) => {
  //   setExpenses((prevState)=>{
  //     return [
  //       expense,
  //       ...prevState
  //     ]
  //   });
  // };

  const [userList, setUserList] = useState([]);

  const addUserHandler = (newUser) => {
    setUserList((prevState)=>{
      return [...prevState, newUser];
    });
  }

  return(
    <div>
      <AddUser onAddUser={addUserHandler}></AddUser>
      <UserList users={userList}></UserList>
    </div>
  );


  // return (
  //   <div>
  //     <NewExpense onAddExpense = {addExpenseHandler}></NewExpense>
  //     <Expenses expenses={expenses}></Expenses>
  //   </div>
  // );
}

export default App;
