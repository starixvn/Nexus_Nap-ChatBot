import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label, Input } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import Labels from '../../common/Labels';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            settingType: 1
        };
    }

    componentDidMount() {
        this.submitQuery();
    }

    handleSettingTypeChange = event =>
        this.setState({
            settingType: event.target.value
        });

    getParams = () => {
        return {
            jobId: parseInt(this.state.settingType, 10),
            pageIndex: 1,
            pageSize: 10
        };
    }

    submitHandler = e => {
        e.preventDefault();
        this.submitQuery();
    }

    submitQuery = () => {
        const params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    renderSettingTypeOption = item => <option key={item.id} value={item.id} >{item.name}</option>

    render() {
        return (
            <Form className="form-filter">
                <FormGroup>
                    <Label className="form-label">{Labels.MappingSettingType}</Label>
                    <Input type="select"
                        name="settingType"
                        onChange={this.handleSettingTypeChange}
                        value={this.state.settingType}
                    >
                        {this.props.settingTypes.map(this.renderSettingTypeOption)}

                    </Input>
                </FormGroup>
                <button className="btn-submit" onClick={this.submitHandler}>{Labels.Submit}</button>
            </Form>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { settingTypes, getJobInfo, gridParams } = state;
    return {
        settingTypes,
        gridParams,
        getJobInfo
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(Filter);