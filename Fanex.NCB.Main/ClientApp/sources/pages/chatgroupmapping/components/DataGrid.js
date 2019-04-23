import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import { UncontrolledTooltip } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import CommonFunctions from '../../common/CommonFunctions';
import { PAGING_SIZES } from '../../constants/constants';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';

export class DataGrid extends Component {
    constructor(props) {
        super(props);
    }   

    onPaging = (pageIndex, pageSize) => {
        let params = { ...this.props.gridParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    openEditPopup = (item) => {
        if(!CommonFunctions.isBotRemoved(item.mappingStatusID)){
            this.props.filterRecipients(this.props.settingData.recipients, item.recipientTypeID);
            this.props.openEditPopup(item);
        }
    }

    buildHeader = () => {
        return (
            <Row style={{ height: 32 }}>
                <Cell colWidth={30} header>{Labels.NumberCol}</Cell>
                <Cell colWidth={100} header>{Labels.BotName}</Cell>
                <Cell colWidth={200} header>{Labels.GroupName}</Cell>
                <Cell colWidth={110} header>{Labels.ChatGroupType}</Cell>
                <Cell colWidth={100} header>{Labels.RecipientType}</Cell>
                <Cell colWidth={130} header>{Labels.Recipient}</Cell>
                <Cell colWidth={100} header>{Labels.LicenseeStatus}</Cell>
                <Cell colWidth={100} header>{Labels.MappingStatus}</Cell>
                <Cell colWidth={250} header>{Labels.Remark}</Cell>
                <Cell colWidth={80} header>{Labels.Action}</Cell>
            </Row>);
    };

    getLicenseeStatus = (item) =>  {
        return item.recipientTypeName ? CommonFunctions.getLicenseeStatus(item.recipientStatusNames) : '';
    }

    getEditClassNames = (item) =>  {
        return CommonFunctions.isBotRemoved(item.mappingStatusID) ? "icon-edit edit-disabled" : "icon-edit";
    }

    buildBody = (data, pageIndex, pageSize) => {
        let rows = [];
        let startIndex = (pageIndex - 1) * pageSize + 1;
        data.map((item, index) => {
            rows.push(<Row key={`body${index}`} >
                <Cell className="text-center">{index + startIndex}</Cell>
                <Cell>{item.botName}</Cell>
                <Cell>{item.groupName}</Cell>
                <Cell>{item.chatGroupTypeName}</Cell>
                <Cell>{item.recipientTypeName}</Cell>
                <Cell>{item.recipientNames}</Cell>
                <Cell>{this.getLicenseeStatus(item)}</Cell>
                <Cell><span className={CommonFunctions.getMappingClass(item.mappingStatusID)}>{item.mappingStatusName}</span></Cell>
                <Cell>{item.remark}</Cell>
                <Cell className="text-center col-action">
                    <div>
                        <span className={this.getEditClassNames(item)} id={"edit-" + index} onClick={() => this.openEditPopup(item)} />                        
                    </div>
                </Cell>
            </Row>);
        });
       
        return rows;
    }   

    render() {
        if (!this.props.gridData || !this.props.gridData.data) {
            return null;
        }

        const data = this.props.gridData.data;
        const count = this.props.gridData.count;
        const pageIndex = this.props.gridParams.pageIndex;
        const pageSize = this.props.gridParams.pageSize;

        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: count,
            PageList: PAGING_SIZES
        };

        if (data.length === 0) {
            return <NoDataTitle />;
        }

        return (
            <div className="table-container-div">
                <Table
                    width={1400}
                    autoWidth={false}
                    header={this.buildHeader()}
                    body={this.buildBody(data, pageIndex, pageSize)}
                    isPaging
                    pageOption={pagingOption}
                    onPaging={this.onPaging}
                    containerPadding={0}
                />
            </div>
        );       
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { gridParams, gridData, settingData } = state;
    return Object.assign({}, { gridParams, gridData, settingData });
}

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);
