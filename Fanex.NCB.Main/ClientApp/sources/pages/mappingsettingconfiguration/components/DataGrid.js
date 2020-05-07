import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';

export class DataGrid extends Component {
    constructor(props) {
        super(props);
    }

    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.gridParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    buildHeader = isHost => {
        return (
            <Row >
                <Cell colWidth={5} header>{Labels.NumberCol}</Cell>
                <Cell colWidth={15} header>{Labels.NotificationType}</Cell>
                <Cell colWidth={15} header>{Labels.NotificationTemplate}</Cell>
                <Cell colWidth={10} header>{Labels.System}</Cell>
                <Cell colWidth={10} header>{Labels.RecipientType}</Cell>
                <Cell colWidth={10} header>{Labels.MsgContentsType}</Cell>
                <Cell colWidth={15} header>{Labels.RequiredParams}</Cell>
                <Cell colWidth={5} header>{Labels.IsTest}</Cell>
                {
                    isHost && <Cell colWidth={5} header>{Labels.Action}</Cell>
                }
            </Row>);
    };

    buildBody = isHost => {
        const { pageIndex, pageSize } = this.props.gridParams;

        this.startIndex = (pageIndex - 1) * pageSize + 1;

        if (this.props.gridData.data.length > 0) {
            return this.props.gridData.data.map((item, index) => this.renderRow(item, index, isHost));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="9"><NoDataTitle /></Cell></Row>];
    }

    handleEditMapping = e => {
        const templateId = parseInt(e.target.dataset.templateid, 10);
        const msgContentsType = parseInt(e.target.dataset.msgcontentstype, 10);
        this.props.getMapping(templateId, msgContentsType, this.props.jobInfo.id);
    }

    handleDeleteMapping = e => {
        const templateId = parseInt(e.target.dataset.templateid, 10);
        const msgContentsType = parseInt(e.target.dataset.msgcontentstype, 10);
        const notificationTypeName = e.target.dataset.typename;
        const notificationTemplateName = e.target.dataset.templatename;
        this.props.openDeleteMappingPopup({ templateId, msgContentsType, notificationTemplateName, notificationTypeName });
    }

    renderRow = (item, index, isHost) => (
        <Row key={index + this.startIndex} >
            <Cell className="text-center">{index + this.startIndex}</Cell>
            <Cell>{item.notificationTypeName}</Cell>
            <Cell>{item.notificationTemplateName}</Cell>
            <Cell>{item.systemName}</Cell>
            <Cell>{item.recipientTypeName}</Cell>
            <Cell>{item.msgContentsName}</Cell>
            <Cell>{item.requiredParams}</Cell>
            <Cell className="text-center">
                {
                    item.isTest ? <span className='icon-check'></span> : null
                }
            </Cell>
            {
                isHost &&
                <Cell className="text-center">
                    <>
                        <span className="icon-edit"
                            data-templateid={item.notificationTemplateID}
                            data-msgcontentstype={item.msgContentsType}
                            onClick={this.handleEditMapping} />
                        {
                            item.unRemovable ?
                            <span className="icon-delete icon-disabled" /> :
                            <span className="icon-delete"
                                data-templateid={item.notificationTemplateID}
                                data-msgcontentstype={item.msgContentsType}
                                data-typename={item.notificationTypeName}
                                data-templatename={item.notificationTypeName}
                                onClick={this.handleDeleteMapping} />
                        }
                    </>
                </Cell>
            }
        </Row>
    )

    render() {
        if (!this.props.gridData || !this.props.gridData.data) {
            return null;
        }

        const { total } = this.props.gridData;
        const { pageIndex, pageSize } = this.props.gridParams;
        const { haveHostPermission } = this.props;

        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: total,
            PageList: [10, 20, 50, 100, 500]
        };

        return (
            <Table
                autoWidth={false}
                width={1620}
                header={this.buildHeader(haveHostPermission)}
                body={this.buildBody(haveHostPermission)}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={50}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, gridData, jobInfo, openDeleteMappingPopup } = state;
    return {
        gridParams,
        gridData,
        jobInfo,
        openDeleteMappingPopup
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);