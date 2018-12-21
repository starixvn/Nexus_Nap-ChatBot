import React, { Component } from 'react';
import { connect } from 'react-redux';
import Header from './Header';
import "./content.scss";

class Content extends Component {
  constructor(props){
    super(props);   
  }  

  render() {
    return (
      <div className="main-container">
            <Header collapseExpandHandler={this.props.collapseExpandHandler}></Header>
            <div className="content ">
                <iframe className="main-content" src={this.props.contentUrl}>                
                </iframe>                           
            </div>
        </div>    
    );    
  }
} 

export default connect()(Content);
