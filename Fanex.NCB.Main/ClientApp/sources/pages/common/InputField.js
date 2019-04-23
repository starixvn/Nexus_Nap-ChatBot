import React, { Component } from 'react';
import { Input, FormFeedback } from 'reactstrap';
import './InputField.scss';

export class InputField extends Component {
  constructor(props) {
    super(props);

    this.state = {
        value: props.initialValue
    };
  }  

  onChange = (e) => {    
    this.setState(
        {
            value: e.target.value
        },
        () => {
            this.props.onChange({
                value: this.state.value,
                isValid: this.isValid()
            });
        }
    );
  }  

  isValid = () => {
    return this.state.value !== '';
  }

  render() {
    return (
      <div className="input-field">
        <Input  value={this.state.value} 
            onChange={this.onChange}
            invalid={this.props.isSummited && !this.isValid()}
        />
        { this.props.isSummited && !this.isValid() && <FormFeedback>This field is required.</FormFeedback> }                    
      </div>
    );
  }
}
