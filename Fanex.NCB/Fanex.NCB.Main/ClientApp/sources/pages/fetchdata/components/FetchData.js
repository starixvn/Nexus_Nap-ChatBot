import React from 'react';
import { connect } from 'react-redux';

const FetchData = props => (
  <div>
    <h1>Fetch data</h1>  
  </div>
);

export default connect()(FetchData);
