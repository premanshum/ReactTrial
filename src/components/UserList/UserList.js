import React from 'react';
import Card from '../UI/Card';

import classes from "./UserList.module.css";

const UserList = (props) =>{    

    return (
        <Card>
            <ul>
            {
                props.users.map(item=>{
                  return(
                    <li>
                        {item.userName} is {item.userAge} old.
                    </li>);
                })
            }
            </ul>
        </Card>
    );
};

export default UserList;