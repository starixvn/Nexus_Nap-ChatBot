import React from 'react';
import { connect } from 'react-redux';
import './NewNotification.scss';

const NewNotification = props => (
  <div>
    <h1>New Notification updated</h1>  
    <div className="content">content</div>
  </div>
);

export default connect()(NewNotification);
