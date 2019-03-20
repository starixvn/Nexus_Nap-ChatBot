import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label } from 'reactstrap';
import { InputField } from '../../common/InputField';
import { SelectField } from '../../common/SelectField';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { MultipleSelectField } from '../../common/MultipleSelectField';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            botNames: {
                value: ""
            },
            recipientTypes: {
                value: ""
            },
            recipient: {
                value: ""
            },
            mappingStatuses: {
                value: ""
            },
        };
    }

    componentWillMount() {
        this.props.getSettings();
        this.props.filterRecipients([], 0);
    }  

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue
        });
    }

    getParams = () => {              
        return {
            botNames: this.state.botNames.value,
            recipientTypeIds: this.state.recipientTypes.value,
            recipientName: this.state.recipient.value,
            mappingStatusIds: this.state.mappingStatuses.value,
            pageIndex: 1,
            pageSize: 5
        };
    }    

    submitHandler = (e) => {
        e.preventDefault();

        let params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }
  
    render() {
        return (
            <Form className="form-filter">
                <FormGroup>
                    <Label className="form-label">Bot Name</Label>
                    <MultipleSelectField
                        id={"botNames"}
                        width={200}
                        initialValue={this.state.botNames.value}
                        data={this.props.settingData.botNames}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        noneSelectedLabel={"All"}
                        onChange={(value) => this.onChange("botNames", value)}
                    />       
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Recipient Type</Label>
                    <MultipleSelectField
                        id={"recipientTypes"}
                        width={200}
                        initialValue={this.state.recipientTypes}
                        data={this.props.settingData.recipientTypes}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        noneSelectedLabel={"All"}
                        onChange={(value) => this.onChange("recipientTypes", value)}
                    />       
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Recipient</Label>
                    <InputField initialValue={this.state.recipient.value}
                        isSummited={this.state.isSummited}
                        onChange={(value) => this.onChange("recipient", value)} /> 
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Mapping Status</Label>
                    <MultipleSelectField
                        id={"mappingStatuses"}
                        width={200}
                        initialValue={this.state.mappingStatuses}
                        data={this.props.settingData.mappingStatuses}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        noneSelectedLabel={"All"}
                        onChange={(value) => this.onChange("mappingStatuses", value)}
                        />
                </FormGroup>
                <button className="btn-submit" onClick={this.submitHandler}>Submit</button>
            </Form>            
        );
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { settingData} = state;
    return Object.assign({}, { settingData });
}

export default connect(mapStateToProps, mapDispatchToProps)(Filter);