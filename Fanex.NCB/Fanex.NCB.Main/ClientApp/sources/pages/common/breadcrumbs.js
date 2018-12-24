import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Breadcrumb, BreadcrumbItem } from 'reactstrap';

class NCBBreadcrum extends Component {
  constructor(props){
    super(props);   
  }  

  render() {
      const {chatbotName, pageTitle} = this.props;

    return (
      <Breadcrumb className="nav-customized">
        <BreadcrumbItem>{chatbotName}</BreadcrumbItem>
        <BreadcrumbItem active>{pageTitle}</BreadcrumbItem>
      </Breadcrumb>
    );    
  }
} 

export default connect()(NCBBreadcrum);
