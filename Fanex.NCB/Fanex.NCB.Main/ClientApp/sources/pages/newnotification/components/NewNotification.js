import React from 'react';
import { connect } from 'react-redux';

const NewNotification = props => (
  <div>
    <h1>New Notification</h1>  
  </div>
);

export default connect()(NewNotification);
