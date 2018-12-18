import React, { Component } from 'react';
import { connect } from 'react-redux';
import Menu from './Menu';
import './Content.scss';

class Content extends Component{
  constructor(props){
    super(props);

    this.state = {
      currentUrl: "/NewNotification/Index"
    }

    this.onMenuChange = this.onMenuChange.bind(this);
  }

  onMenuChange(url){
    this.setState({
      currentUrl: url
    })
  }

  render() {
    return (
      <div className="body-content">
            <div className="left-panel">
                <Menu onChange={this.onMenuChange}></Menu>
            </div>
            <div className="content ">
                <iframe src={this.state.currentUrl}>                
                </iframe>                           
            </div>
        </div>    
    );    
  }
} 

export default connect()(Content);
