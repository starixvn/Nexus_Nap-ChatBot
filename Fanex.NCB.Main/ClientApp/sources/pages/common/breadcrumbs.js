import React, { Component } from 'react';
import { Breadcrumb, BreadcrumbItem } from 'reactstrap';

export class NCBBreadcrum extends Component {
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