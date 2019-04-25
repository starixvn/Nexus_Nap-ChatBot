import React, { Component } from 'react';
import { Input, FormFeedback } from 'reactstrap';
import './TemplateTextField.scss';

export class TemplateTextField extends Component {
    constructor(props) {
        super(props);

       
    }

    render() {
        return (
            <div className="template-text-field" dangerouslySetInnerHTML={{ __html: this.props.value }} />            
        );
    }
}
