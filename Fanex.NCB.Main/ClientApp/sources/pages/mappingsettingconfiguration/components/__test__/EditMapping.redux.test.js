jest.mock('react-redux');
jest.mock('redux');
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';

describe('connect', () => {
    let mapStateToProps, mapDispatchToProps;
    const connectMock = jest.fn();

    beforeAll(() => {
        connect.mockImplementation((mapStateToPropsFunc, mapDispatchToPropsFunc) => {
            mapStateToProps = mapStateToPropsFunc;
            mapDispatchToProps = mapDispatchToPropsFunc;
            return connectMock;
        });
        require('../EditMapping').default;
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    it('props should be mapped correctly', () => {
        // Arrange
        const expected = {
            editMappingPopup: { show: true },
            editMappingResult: { show: true },
            notificationTypes: '1,2,3',
            msgContentsTypes: '1,2,3',
            notificationTypeRef: 1,
            jobInfo: 1,
            settingTypes: { checked: true }
        };
        const state = {
            editMappingPopup: { show: true },
            editMappingResult: { show: true },
            notificationTypes: '1,2,3',
            msgContentsTypes: '1,2,3',
            notificationTypeRef: 1,
            jobInfo: 1,
            settingTypes: { checked: true }
        };

        // Act
        const actual = mapStateToProps(state);

        // Assert
        expect(actual).toEqual(expected);
    });

    it('bindActionCreators should be called with correct parameter', () => {
        // Arrange
        const mockBindAction = jest.fn();
        bindActionCreators.mockImplementationOnce(mockBindAction);
        const dispatch = {};

        // Act
        mapDispatchToProps(dispatch);

        // Assert
        expect(mockBindAction).toHaveBeenCalled();
    });
});