import React, { useState, useEffect, useReducer } from 'react';
import classes from './Login.module.css';
import Card from '../UI/Card';
import Button from '../UI/Button';

const formStateReducer = (state, action)=>{

  console.log(state);
  if(action.type === 'EMAIL_INPUT'){
    return {...state,
      emailValue : action.value,
      isEmailValid : action.value.includes('@'),
      isFormValid : state.isPasswordValid && action.value.includes('@')}
  }

  if(action.type === 'PASSWORD_INPUT'){
    return {...state,
      passwordValue : action.value,
      isPasswordValid : action.value.length >= 7,
      isFormValid : state.isEmailValid && action.value.length >= 7}
  }

  return {
    emailValue : '',
    passwordValue : '',
    isEmailValid: false,
    isPasswordValid: false,
    isFormValid : false
  };
};

const initialFormState = {
  emailValue : '',
  passwordValue : '',
  isEmailValid: false,
  isPasswordValid: false,
  isFormValid : false
};

const Login = (props) => {

  // Reducer
  const [formState, setFormState] = useReducer(formStateReducer, initialFormState);

  // State
  // const [emailIsValid, setEmailIsValid] = useState();
  // const [passwordIsValid, setpasswordIsValid] = useState();
  // const [enteredEmail, setEnteredEmail] = useState('');
  // const [enteredPassword, setEnteredPassword] = useState('');
  // const [formIsValid, setFormIsValid] = useState();

  // useEffect(()=>{
  //   if(emailIsValid && passwordIsValid){
  //     setFormIsValid(true)
  //   }
  // }, [emailIsValid, passwordIsValid])

  // useEffect(()=>{
  //   if(enteredEmail.length>0 && enteredEmail.includes('@')){
  //     setEmailIsValid(true);
  //   }
  // }, [enteredEmail]);

  // useEffect(()=>{
  //   if(enteredPassword.length >=7){
  //     setpasswordIsValid(true);
  //   }
  // }, [enteredPassword]);

  // Handlers
  const emailChangeHandler = (event)=>{
    //setEnteredEmail(event.target.value);
    setFormState({type:'EMAIL_INPUT', value: event.target.value});
  };

  const validateEmailHandler = (event)=>{
    //setEmailIsValid(enteredEmail.length !== 0 && enteredEmail.includes('@'));
  };

  const passwordChangeHandler = (event)=>{
    //setEnteredPassword(event.target.value);
    setFormState({type:'PASSWORD_INPUT', value: event.target.value})
  };

  const validatePasswordHandler = (event)=>{
    //setpasswordIsValid(enteredPassword.length >= 7);
  };

  const submitHandler = (event)=>{
    event.preventDefault();
    if(formState.isFormValid){
      props.onLogin();
    }
  };
  // Handlers Ends

  return (
    <Card className={classes.login}>
      <form onSubmit={submitHandler}>
      <div
          className={`${classes.control} ${
            formState.isEmailValid === false ? classes.invalid : ''
          }`}
        >
          <label htmlFor="email">E-Mail</label>
          <input
            type="email"
            id="email"
            value={formState.emailValue}
            onChange={emailChangeHandler}
            onBlur={validateEmailHandler}
          />
        </div>
        <div
          className={`${classes.control} ${
            formState.isPasswordValid === false ? classes.invalid : ''
          }`}
        >
          <label htmlFor="password">Password</label>
          <input
            type="password"
            id="password"
            value={formState.passwordValue}
            onChange={passwordChangeHandler}
            onBlur={validatePasswordHandler}
          />
        </div>
        <div className={classes.actions}>
          <Button type="submit" className={classes.btn} disabled={!formState.isFormValid}>
            Login
          </Button>
        </div>
      </form>
    </Card>);
}

export default Login;