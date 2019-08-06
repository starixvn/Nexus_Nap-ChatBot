import React, { Component } from 'react';
import { Input, FormFeedback } from 'reactstrap';
import Labels from '../common/Labels';

import './InputField.scss';

export class InputField extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="input-field">
                <Input {...this.props} />
                {this.props.invalid && <FormFeedback>{Labels.FieldRequired}</FormFeedback>}
            </div>
        );
    }
}