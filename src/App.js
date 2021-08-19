import Expenses from "./components/Expenses/Expenses";
import NewExpense from "./components/NewExpense/NewExpense";
import AddUser from "./components/AddUser/AddUser";
import UserList from './components/UserList/UserList';
import Login from './components/Login/Login';
import React, { useEffect, useState } from "react";
import MainHeader from "./components/MainHeader/MainHeader";
import Home from './components/Home/Home';

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

  const [feature, setFeature] = useState('3');
  const [expenses, setExpenses] = useState(DUMMY_EXPENSES);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  
  

  useEffect(()=>{
    const storedUserLoginInfo = localStorage.getItem('isLoggedIn');
    if(storedUserLoginInfo === '1'){
      setIsLoggedIn(true);
    }
  }, []);

  const loginHandler = (email, password) => {
    // We should of course check email and password
    // But it's just a dummy/ demo anyways
    setIsLoggedIn(true);
    localStorage.setItem('isLoggedIn', '1');
  };

  const logoutHandler = () => {
    localStorage.setItem('isLoggedIn', '0');
    setIsLoggedIn(false);
  };

  const addExpenseHandler = (expense) => {
    setExpenses((prevState)=>{
      return [
        expense,
        ...prevState
      ]
    });
  };

  const [userList, setUserList] = useState([]);

  const addUserHandler = (newUser) => {
    setUserList((prevState)=>{
      return [...prevState, newUser];
    });
  }

  const selectChangeHandler = (event)=>{
    event.preventDefault();
    setFeature(event.target.value)
  }

  return(
    <React.Fragment>
      <div>
        <select onChange={selectChangeHandler}>
          <option id='feat1' value='1' >1. Expense</option>
          <option id='feat2' value='2'>2. Data Entry</option>
          <option id='feat3' value='3' selected>3. Login</option>
        </select>
      </div>
      {
        feature === '1' && 
        <div>
          <NewExpense onAddExpense = {addExpenseHandler}></NewExpense>
          <Expenses expenses={expenses}></Expenses>
        </div>
      }
      {
        feature === '2' && 
        <div>
          <AddUser onAddUser={addUserHandler}></AddUser>
          <UserList users={userList}></UserList>
        </div>
      }
      {
        feature === '3' && 
        <React.Fragment>
          <MainHeader isAuthenticated={isLoggedIn} onLogout={logoutHandler} />
          <main>
            {!isLoggedIn && <Login onLogin={loginHandler} />}
            {isLoggedIn && <Home onLogout={logoutHandler} />}
          </main>
        </React.Fragment>
      }
    </React.Fragment>
  );
}

export default App;
